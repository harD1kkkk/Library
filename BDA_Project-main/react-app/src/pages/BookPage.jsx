import "../styles/pages/BookPage.scss";
import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import { Button, Input, Typography, Card, Rate, Modal, message } from "antd";
import { fetchBooksItems, selectAllBooks } from "../store/slices/bookSlice";
import {
  borrowBook,
  selectBorrowedBooks,
} from "../store/slices/borrowBookSlice";
import { selectUser } from "../store/slices/userSlice";
import {
  fetchFeedbacks,
  submitFeedback,
  selectFeedbackByBook,
} from "../store/slices/feedbackSlice";

const { Title, Paragraph } = Typography;

const BookPage = () => {
  const { id } = useParams();
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const books = useSelector(selectAllBooks);
  const book = books.find((p) => p.id === Number(id));
  const user = useSelector(selectUser);
  const borrowedBooks = useSelector(selectBorrowedBooks);
  const feedbacks = useSelector((state) => selectFeedbackByBook(state, id));
  console.log(feedbacks);
  const [feedback, setFeedback] = useState("");
  const [rating, setRating] = useState(0);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!books.length) {
      dispatch(fetchBooksItems()).finally(() => setLoading(false));
    } else {
      setLoading(false);
    }
    if (book) {
      dispatch(fetchFeedbacks(book.id));
    }
  }, [books, book, dispatch]);

  useEffect(() => {
    if (book) dispatch(fetchFeedbacks(book.id));
  }, [book, dispatch]);

  const handleFeedbackChange = (e) => setFeedback(e.target.value);

  const handleFeedbackSubmit = (e) => {
    e.preventDefault();
    if (!borrowedBooks.includes(book.id)) {
      message.error("You can only leave feedback after borrowing this book.");
    } else {
      dispatch(
        submitFeedback({
          bookId: book.id,
          feedback: { rating, message: feedback, userId: user.id },
        })
      );
      setFeedback("");
      setRating(0);
      message.success("Feedback submitted successfully.");
    }
  };

  const handleBorrowClick = () => {
    if (!user) {
      setIsModalOpen(true);
      return;
    }
    dispatch(borrowBook(book.id));
    message.success("Book borrowed successfully!");
  };

  const handleModalOk = () => {
    setIsModalOpen(false);
    navigate("/library/sign-in");
  };

  const handleModalCancel = () => setIsModalOpen(false);

  if (loading) {
    return <div className="text-center">Book loading...</div>;
  }

  if (!book) {
    return <div className="text-center text-red-500">Book not found</div>;
  }

  return (
    <div className="book-page">
      <Card className="book-page__card">
        <div className="flex justify-center">
          <img
            src={book.image}
            alt={book.title}
            className="book-page__card__image"
          />
        </div>
        <div className="book-page__card__content">
          <Title level={1} className="book-page__card__content__title">
            {book.title}
          </Title>
          <Paragraph className="book-page__card__content__paragraph">
            {book.description}
          </Paragraph>

          <div className="book-page__card__ratings">
            <Rate
              allowHalf
              disabled
              value={Math.round(book.averageRating * 2) / 2}
            />
            <span className="book-page__card__total-reviews">
              ({book.totalReviews} reviews)
            </span>
          </div>

          <Button
            type="primary"
            className="book-page__card__content__button"
            onClick={handleBorrowClick}
            disabled={!user || borrowedBooks.includes(book.id)}
          >
            {borrowedBooks.includes(book.id) ? "Already Borrowed" : "Borrow Book"}
          </Button>
        </div>
      </Card>

      <Card className="book-page__feedback">
        <div className="book-page__feedback__content">
          <Title level={2} className="book-page__feedback__title">
            Leave Feedback
          </Title>
          <form
            onSubmit={handleFeedbackSubmit}
            className="book-page__feedback__form"
          >
            <Input.TextArea
              value={feedback}
              onChange={handleFeedbackChange}
              placeholder="Write your feedback here..."
              rows={4}
              required
              disabled={!borrowedBooks.includes(book.id)}
            />
            <Rate
              allowHalf
              value={rating}
              onChange={setRating}
              className="mb-4"
              disabled={!borrowedBooks.includes(book.id)}
            />
            <Button
              type="primary"
              htmlType="submit"
              disabled={!borrowedBooks.includes(book.id)}
              className="book-page__feedback__button"
            >
              Submit Feedback
            </Button>
          </form>

          <div className="book-page__feedback__list">
            <Title level={2} className="book-page__feedback__title">
              Existing Feedbacks
            </Title>
            <ul className="book-page__feedback__list__list">
              {feedbacks.map((f) => (
                <li key={f.id} className="book-page__feedback__list__item">
                  <span className="font-semibold">
                    {f.userId ? "You: " : "Anonymous: "}
                  </span>
                  <br />
                  {f.message} <br /><Rate allowHalf disabled value={f.rating} />
                </li>
              ))}
            </ul>
          </div>
        </div>
      </Card>

      <Modal
        title="Sign In Required"
        open={isModalOpen}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        okText="Go to Sign-In"
        cancelText="Cancel"
        className="book-page__modal"
      >
        <p className="book-page__modal__text">
          You need to be signed in to borrow this book. Do you want to sign in
          now?
        </p>
      </Modal>
    </div>
  );
};

export default BookPage;
